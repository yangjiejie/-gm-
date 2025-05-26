using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GmAttribute : Attribute
{
    public int idGroup { get; set; }
    
    public string desc { get; set; }
    public GmAttribute(int idGroup,string desc)
    {
        this.idGroup = idGroup;
        
        this.desc = desc;
    }
}