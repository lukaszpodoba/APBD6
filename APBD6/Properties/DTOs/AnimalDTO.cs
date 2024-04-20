using System.ComponentModel.DataAnnotations;

namespace APBD6.Properties.DTOs;

public record GetAllAnimals(int id, string name, string description, string category, string area);
public record CreateAnimalRequest(string name, string description, string category, string area);
