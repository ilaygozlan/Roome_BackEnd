using Microsoft.OpenApi.Any;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class PropertyType{
        int id;
        string propertyTypeName=" ";

        public PropertyType(int id, string propertyTypeName)
        {
            Id = id;
            PropertyTypeName = propertyTypeName;
        }
        public PropertyType(){
            
        }

        public int Id { get => id; set => id = value; }
        public string PropertyTypeName { get => PropertyTypeName; set => PropertyTypeName = value; }
    }
}