using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<AppUser, MemberDto>()
      .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()));
    CreateMap<Photo, PhotoDto>();

    // TODO: Remove the ignore mappings when update forms are implemented
    CreateMap<MemberUpdateDto, AppUser>()
      .ForMember(d => d.Id, o => o.Ignore())
      .ForMember(d => d.UserName, o => o.Ignore())
      .ForMember(d => d.PasswordHash, o => o.Ignore())
      .ForMember(d => d.PasswordSalt, o => o.Ignore())
      .ForMember(d => d.DateOfBirth, o => o.Ignore())
      .ForMember(d => d.KnownAs, o => o.Ignore())
      .ForMember(d => d.Created, o => o.Ignore())
      .ForMember(d => d.LastActive, o => o.Ignore())
      .ForMember(d => d.Gender, o => o.Ignore())
      .ForMember(d => d.IsPredator, o => o.Ignore())
      .ForMember(d => d.Bio, o => o.Ignore())
      .ForMember(d => d.Photos, o => o.Ignore());
  }
}
