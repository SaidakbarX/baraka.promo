using baraka.promo.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.PromoEntities
{
    public class PromoGroup : Entity
    {
        public PromoGroup()
        {

        }
        public PromoGroup(int order, string Name, string user, string desc = "") : base(user)
        {
            Order = order;
            this.Name = Name;
            Description = desc;
        }
        public int Id { get; set; }
        public int Order { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }

        public void Update(string name, string desc, string user)
        {
            this.Name = name;
            this.Description = desc;
            Modified(user);
        }
    }
}
