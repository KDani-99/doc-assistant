using System.ComponentModel.DataAnnotations;
using DocAssistant_Common.Attributes;

namespace DocAssistant_Common.Models
{
    public abstract class Entity
    {
        public virtual long Id { get; set; }
    }
}