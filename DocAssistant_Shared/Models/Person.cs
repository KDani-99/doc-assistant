using System.ComponentModel.DataAnnotations;
using DocAssistant_Common.Attributes;

namespace DocAssistant_Common.Models
{
    public abstract class Person : Entity
    {
        [CommonValidation("^[^~!@#$%^&*\\(\\)-=_+\\{\\}\\[\\];\\\":<>\\/?,.|\\\\]{1,50}$")] public virtual string FirstName { get; set; }
        [CommonValidation("^[^~!@#$%^&*\\(\\)-=_+\\{\\}\\[\\];\\\":<>\\/?,.|\\\\]{1,50}$")] public virtual string LastName { get; set; }

        public override string ToString()
        {
            return $"[Id={Id};FirstName={FirstName};LastName={LastName}]";
        }
        
        protected bool Equals(Person other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}