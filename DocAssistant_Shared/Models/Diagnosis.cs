using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocAssistant_Common.Attributes;

namespace DocAssistant_Common.Models
{
    public sealed class Diagnosis : Entity
    {
        [Key] public override long Id { get; set; }
        [Fixed] [ForeignKey("PatientId")] public long PatientId { get; set; }
        [Required] [CommonValidation("^.{1,50}$")] public string Title { get; set; }
        [Required] [CommonValidation("^.{1,500}$")] public string Description { get; set; }
        [Required] public DateTime Date { get; set; }

        private bool Equals(Diagnosis other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Diagnosis other && Equals(other);
        }
    }
}