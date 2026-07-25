using API.OneOfTypes;
using AutoMapper;
using DTO.Movies;
using DTO.SearchQueries;
using Models;
using OneOf;
using OneOf.Types;
using Repositories;
using Repositories.Interfaces;

namespace API.Movies;

public class MovieService(IMapper mapper, IRepositoryManager repositoryManager)
{
    public async Task<List<MovieResponse>> GetMoviesAsync(MovieSearchQuery query)
    {
        var movies = await repositoryManager.Movies.Search(query);
        
        var moviesDto = mapper.Map<List<MovieResponse>>(movies);
        foreach(var movie in moviesDto)
            movie.Stock = await GetMovieStock(movie.Id);

        return moviesDto;
    }
    
    public async Task<OneOf<MovieResponse, MovieAlreadyExists>> AddMovieAsync(AddMovieRequest request)
    {
        var movie = mapper.Map<Movie>(request);

        var movieExists = await repositoryManager.Movies.MovieExistsAsync(movie);
        if (movieExists)
            return new MovieAlreadyExists();

        await repositoryManager.Movies.CreateAsync(movie);
        
        var movieResponse = mapper.Map<MovieResponse>(movie);
        movieResponse.Stock = await GetMovieStock(movie.Id);
        
        await repositoryManager.Movies.SaveChangesAsync();
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await repositoryManager.Movies.GetByIdAsync(id, x => x.Id == id);
        
        if(movie == null)
            return new MovieNotFound();
        
        var movieResponse = mapper.Map<MovieResponse>(movie);
        movieResponse.Stock = await GetMovieStock(movie.Id);
        return movieResponse;
    }

    private async Task<int> GetMovieStock(Guid movieId)
    {
        var movieExists = await repositoryManager.Movies.MovieExistsAsync(movieId);
        if (!movieExists)
            return 0;

        var totalInventory = await repositoryManager.InventoryRecords.GetTotalAmount(movieId, DateOnly.FromDateTime(DateTime.Now));
        var totalRentedMovies = await repositoryManager.Rentals.GetByMovieIdAsync(movieId);
        var totalMoviesNotReturned = totalRentedMovies.Count(x => x.DateReturned == null);
        return totalInventory - totalMoviesNotReturned;
    }

    public async Task<OneOf<MovieResponse, MovieAlreadyExists, NotFound>> UpdateAsync(Guid id, UpdateMovieRequest request)
    {
        var movie = await repositoryManager.Movies.GetByIdAsync(id);
        if (movie == null)
            return new NotFound();

        var existingMovies = await repositoryManager.Movies.Search(new MovieSearchQuery
        {
            Name = request.Name,
        });
        
        // Ako bilo koji postojeci film ima ISTI DATUM A RAZLICIT ID
        if (existingMovies.Any(x => x.ReleaseDate == request.ReleaseDate && x.Id != id))
            return new MovieAlreadyExists();
        
        // Begin Tracking
        await repositoryManager.Movies.Update(movie);
        
        movie.Name = request.Name;
        movie.ReleaseDate = request.ReleaseDate;

        var currentMovieGenres = await repositoryManager.MovieGenres.GetByMovieId(movie.Id);
        foreach (var currentGenre in currentMovieGenres)
        {
            if (request.GenreIds.Remove(currentGenre.GenreId))
                continue;
            
            await repositoryManager.MovieGenres.Delete(currentGenre);
        }
        
        foreach (var genreId in request.GenreIds)
        {
            var genre = await repositoryManager.Genres.GetByIdAsync(genreId);
            if (genre == null)
                continue;

            var movieGenre = new MovieGenre
            {
                MovieId = movie.Id,
                GenreId = genre.Id,
            };
            await repositoryManager.MovieGenres.CreateAsync(movieGenre);
        }

        await repositoryManager.Movies.SaveChangesAsync();
        
        // Calling it again to get includes
        movie = await repositoryManager.Movies.GetByIdAsync(id, x => x.Genres);
        if (movie == null)
            return new NotFound();
        var movieStock = await GetMovieStock(movie.Id);
        
        var movieDto = mapper.Map<MovieResponse>(movie);
        movieDto.Stock = movieStock;
        return movieDto;
    }
}