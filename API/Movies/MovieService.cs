using API.OneOfTypes;
using AutoMapper;
using DTO.Movies;
using DTO.SearchQueries;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Movies;

public class MovieService(IMapper mapper,
    IMovieRepository movieRepo, 
    IRentalRepository rentalRepo, 
    IInventoryRecordRepository inventoryRepo, 
    IGenreRepository genreRepo, 
    IMovieGenreRepository movieGenreRepo)
{
    public async Task<List<MovieResponse>> GetMoviesAsync(MovieSearchQuery query)
    {
        var movies = await movieRepo.Search(query);
        
        var moviesDto = mapper.Map<List<MovieResponse>>(movies);
        foreach(var movie in moviesDto)
            movie.Stock = await GetMovieStock(movie.Id);

        return moviesDto;
    }
    
    public async Task<OneOf<MovieResponse, MovieAlreadyExists>> AddMovieAsync(AddMovieRequest request)
    {
        var movie = mapper.Map<Movie>(request);

        var movieExists = await movieRepo.MovieExistsAsync(movie);
        if (movieExists)
            return new MovieAlreadyExists();

        await movieRepo.CreateAsync(movie);
        
        var movieResponse = mapper.Map<MovieResponse>(movie);
        movieResponse.Stock = await GetMovieStock(movie.Id);
        
        await movieRepo.SaveChangesAsync();
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await movieRepo.GetByIdAsync(id, x => x.Id == id);
        
        if(movie == null)
            return new MovieNotFound();
        
        var movieResponse = mapper.Map<MovieResponse>(movie);
        movieResponse.Stock = await GetMovieStock(movie.Id);
        return movieResponse;
    }

    private async Task<int> GetMovieStock(Guid movieId)
    {
        var movieExists = await movieRepo.MovieExistsAsync(movieId);
        if (!movieExists)
            return 0;

        var totalInventory = await inventoryRepo.GetTotalAmount(movieId, DateOnly.FromDateTime(DateTime.Now));
        var totalRentedMovies = await rentalRepo.GetByMovieIdAsync(movieId);
        var totalMoviesNotReturned = totalRentedMovies.Count(x => x.DateReturned == null);
        return totalInventory - totalMoviesNotReturned;
    }

    public async Task<OneOf<MovieResponse, MovieAlreadyExists, NotFound>> UpdateAsync(Guid id, UpdateMovieRequest request)
    {
        var movie = await movieRepo.GetByIdAsync(id);
        if (movie == null)
            return new NotFound();

        var existingMovies = await movieRepo.Search(new MovieSearchQuery
        {
            Name = request.Name,
        });
        
        // Ako bilo koji postojeci film ima ISTI DATUM A RAZLICIT ID
        if (existingMovies.Any(x => x.ReleaseDate == request.ReleaseDate && x.Id != id))
            return new MovieAlreadyExists();
        
        // Begin Tracking
        await movieRepo.Update(movie);
        
        movie.Name = request.Name;
        movie.ReleaseDate = request.ReleaseDate;

        var currentMovieGenres = await movieGenreRepo.GetByMovieId(movie.Id);
        foreach (var currentGenre in currentMovieGenres)
        {
            if (request.GenreIds.Remove(currentGenre.GenreId))
                continue;
            
            await movieGenreRepo.Delete(currentGenre);
        }
        
        foreach (var genreId in request.GenreIds)
        {
            var genre = await genreRepo.GetByIdAsync(genreId);
            if (genre == null)
                continue;

            var movieGenre = new MovieGenre
            {
                MovieId = movie.Id,
                GenreId = genre.Id,
            };
            await movieGenreRepo.CreateAsync(movieGenre);
        }

        await movieRepo.SaveChangesAsync();
        
        // Calling it again to get includes
        movie = await movieRepo.GetByIdAsync(id, x => x.Genres);
        if (movie == null)
            return new NotFound();
        var movieStock = await GetMovieStock(movie.Id);
        
        var movieDto = mapper.Map<MovieResponse>(movie);
        movieDto.Stock = movieStock;
        return movieDto;
    }
}