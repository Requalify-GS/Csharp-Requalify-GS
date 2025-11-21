using Microsoft.ML.Data;

namespace Requalify.ML
{
    public class TrainingData
    {
        [LoadColumn(0)] public string CargoAtual { get; set; }
        [LoadColumn(1)] public string SkillPrincipal { get; set; }
        [LoadColumn(2)] public string NivelSkill { get; set; }
        [LoadColumn(3)] public string Formacao { get; set; }
        [LoadColumn(4)] public string AreaInteresse { get; set; }
    }

    public class PredictionResult
    {
        [ColumnName("PredictedLabel")]
        public string PredictedArea { get; set; }
    }
}
