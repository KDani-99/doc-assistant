using DocAssistant_Common.Models;
using DocAssistantWebApi.Database.Factories;

namespace DocAssistantWebApi.Database.Repositories
{
    public class DiagnosisRepository : BaseRepository<Diagnosis>
    {
        public DiagnosisRepository(IDatabaseFactory databaseFactory) : base(databaseFactory) {}
    }
}