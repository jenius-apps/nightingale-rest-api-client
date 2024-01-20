using Nightingale.Core.Interfaces;

namespace Nightingale.Core.Models
{
    public class ApiTest : ModifiableBase, IApiTest, IDeepCloneable
    {
        private string _name;
        private string _code;

        public ApiTest()
        {
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ObjectModified();
                }
            }
        }

        public string CodeContent
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    ObjectModified();
                }
            }
        }

        public object DeepClone()
        {
            var result = new ApiTest
            {
                Name = this.Name,
                CodeContent = this.CodeContent,
                Status = ModifiedStatus.New
            };

            return result;
        }
    }
}
