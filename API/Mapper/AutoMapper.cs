using AutoMapper;
using DTO.Genres;
using DTO.InventoryRecords;
using DTO.MovieGenres;
using DTO.Movies;
using DTO.Rentals;
using Models;

namespace API.Mapper;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        CreateMap<AppUser, RentalUserResponse>().ReverseMap();
        
        CreateMap<Genre, CreateGenreRequest>().ReverseMap();
        CreateMap<Genre, GenreResponse>().ReverseMap();
        CreateMap<Genre, MovieGenreResponse>().ReverseMap();
        
        CreateMap<InventoryRecord, CreateInventoryRecordRequest>().ReverseMap();
        CreateMap<InventoryRecord, InventoryRecordResponse>().ReverseMap();
        CreateMap<InventoryRecord, InventoryRecordDataResponse>().ReverseMap();
        CreateMap<InventoryRecord, InventoryRecordSingularResponse>().ReverseMap();
        
        CreateMap<MovieGenre, AddMovieGenreRequest>().ReverseMap();
        
        CreateMap<Movie, AddMovieRequest>().ReverseMap();
        CreateMap<Movie, MovieResponse>().ReverseMap();
        CreateMap<Movie, UpdateMovieRequest>().ReverseMap();
        CreateMap<Movie, RentalMovieResponse>().ReverseMap();
        
        CreateMap<Rental, RentalDetailResponse>().ReverseMap();
        CreateMap<Rental, RentalResponse>().ReverseMap();
        CreateMap<Rental, RentRequest>().ReverseMap();
        CreateMap<Rental, ReturnRequest>().ReverseMap();
        CreateMap<Rental, UserRentalsResponse>().ReverseMap();
    }
}