using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VideoApp.BL.DALModels;

public partial class Country
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    [DisplayName("Country")]
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
