//Note that in order to use the Serialization reference, you have to add it first
using System.Runtime.Serialization;
using System.Text;

namespace WP71Demo.Model
{
    public class School
    {
        public static readonly string KEY = "school";

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("The school name is ");
            sb.Append(Name);
            sb.Append(", and the adress is ");
            sb.Append(Address.ToString());
            sb.Append(".");
            return sb.ToString();
        }

    }
}
