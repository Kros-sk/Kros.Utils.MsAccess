using Kros.Data;
using System.Data.SqlClient;

namespace Kros.Utils.Examples
{
    public class IdGeneratorExamples
    {
        public void GeneratorExample()
        {
            using (var conn = new SqlConnection())
            {
                var idGeneratorFactory = IdGeneratorFactories.GetFactory(conn);

                var peopleService = new PeopleService(idGeneratorFactory);
                peopleService.GenerateData();
            }
        }

        #region IdGeneratorFactory
        public class PeopleService
        {
            private IIdGeneratorFactory _idGeneratorFactory;

            public PeopleService(IIdGeneratorFactory idGeneratorFactory)
            {
                _idGeneratorFactory = Check.NotNull(idGeneratorFactory, nameof(idGeneratorFactory));
            }

            public void GenerateData()
            {
                using (var idGenerator = _idGeneratorFactory.GetGenerator("people", 1000))
                {
                    for (int i = 0; i < 1800; i++)
                    {
                        var person = new Person()
                        {
                            Id = idGenerator.GetNext()
                        };
                    }
                }
            }
        }
        #endregion
    }

    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
