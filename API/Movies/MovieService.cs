using API.OneOfTypes;
using DTO.Movies;
using DTO.SearchQueries;
using Models;
using OneOf;
using OneOf.Types;
using Repositories.Interfaces;

namespace API.Movies;

public class MovieService(IMovieRepository movieRepo, 
    IRentalRepository rentalRepo, 
    IInventoryRecordRepository inventoryRepo, 
    IGenreRepository genreRepo, 
    IMovieGenreRepository movieGenreRepo)
{
    public async Task<List<MovieResponse>> GetMoviesAsync(MovieSearchQuery query)
    {
        var movies = await movieRepo.Search(query);

        var moviesDto = movies.Select(x => new MovieResponse
        {
            Id = x.Id,
            Name = x.Name,
            ReleaseDate = x.ReleaseDate,
            Genres = x.Genres.Select(genre => new MovieGenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
            }).ToList(),
            Stock = GetMovieStock(x.Id).GetAwaiter().GetResult()
        });
        
        return moviesDto.ToList();
    }
    
    public async Task<OneOf<MovieResponse, MovieAlreadyExists>> AddMovieAsync(AddMovieRequest request)
    {
        var movie = new Movie
        {
            Name = request.Name,
            ReleaseDate = request.ReleaseDate,
        };

        var movieExists = await movieRepo.MovieExistsAsync(movie);
        if (movieExists)
            return new MovieAlreadyExists();

        await movieRepo.CreateAsync(movie);
        var movieResponse = new MovieResponse
        {
            Id = movie.Id,
            Name = movie.Name,
            ReleaseDate = movie.ReleaseDate,
            Genres = [],
        };
        
        await movieRepo.SaveChangesAsync();
        return movieResponse;
    }

    public async Task<OneOf<MovieResponse, MovieNotFound>> GetMovieByIdAsync(Guid id)
    {
        var movie = await movieRepo.GetByIdAsync(id, x => x.Id == id);

        return movie == null
            ? new MovieNotFound()
            : new MovieResponse
            {
                Id = movie.Id,
                Name = movie.Name,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.Genres.Select(x => new MovieGenreResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
                Stock = await GetMovieStock(movie.Id)
            };
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

        // Begin Tracking
        await movieRepo.Update(movie);

        var existingMovies = await movieRepo.Search(new MovieSearchQuery
        {
            Name = request.Name,
        });
        
        // Ako bilo koji postojeci film ima ISTI DATUM A RAZLICIT ID
        if (existingMovies.Any(x => x.ReleaseDate == request.ReleaseDate && x.Id != id))
            return new MovieAlreadyExists();
        
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

        return new MovieResponse
        {
            Id = movie.Id,
            Name = movie.Name,
            ReleaseDate = movie.ReleaseDate,
            Genres = movie.Genres.Select(x => new MovieGenreResponse
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList(),
            Stock = movieStock
        };
    }
}