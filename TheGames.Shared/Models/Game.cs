using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TheGames.Shared.Models;

[DataContract]
public class Game
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public string? Name { get; set; }

    [DataMember]
    public string? Developer { get; set; }

    [DataMember]
    public long? PublisherId { get; set; }

    [DataMember]
    public Publisher? Publisher { get; set; }
}
