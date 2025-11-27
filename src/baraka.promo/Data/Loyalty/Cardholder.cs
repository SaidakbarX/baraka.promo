using baraka.promo.Data.Base;
using baraka.promo.Models.Enums;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Loyalty
{
    public class Cardholder : Entity
    {
        private Cardholder()
        {
            
        }
        public Cardholder(string name, string phone, DateTime? date_of_birth, CardholderType type, string user,string email,CardholderSex sex) : base(user)
        {
            Name = name;
            Phone = phone;
            DateOfBirth = date_of_birth;
            Type = type;
            Email = email;

        }

        public Guid Id { get; private set; }
        [MaxLength(150)]
        public string? Name { get; private set; }
        [MaxLength(50)]
        public string? Phone { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public CardholderType Type { get; private set; }
        public CardholderSex Sex { get; set; }
        [StringLength(50)]
        public string Email { get; set; }

        public void Update(string name, string phone, DateTime? date_birth, CardholderType type, string user,string email,CardholderSex sex)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Phone = phone;
            DateOfBirth = date_birth;
            Type = type;
            Email = email;
            Sex = sex;

            Modified(user);
        }
    }
}
