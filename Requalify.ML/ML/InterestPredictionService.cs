using Microsoft.ML;

namespace Requalify.ML
{
    public class InterestPredictionService
    {
        private readonly PredictionEngine<TrainingData, PredictionResult> _engine;

        public InterestPredictionService()
        {
            var ml = new MLContext();

            // carregar dados diretamente da lista em memória
            var data = ml.Data.LoadFromEnumerable(TrainingDataset.Data);

            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label", nameof(TrainingData.AreaInteresse))
                .Append(ml.Transforms.Text.FeaturizeText("CargoFeats", nameof(TrainingData.CargoAtual)))
                .Append(ml.Transforms.Text.FeaturizeText("SkillFeats", nameof(TrainingData.SkillPrincipal)))
                .Append(ml.Transforms.Text.FeaturizeText("NivelFeats", nameof(TrainingData.NivelSkill)))
                .Append(ml.Transforms.Text.FeaturizeText("FormacaoFeats", nameof(TrainingData.Formacao)))
                .Append(ml.Transforms.Concatenate("Features",
                    "CargoFeats", "SkillFeats", "NivelFeats", "FormacaoFeats"))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(data);

            _engine = ml.Model.CreatePredictionEngine<TrainingData, PredictionResult>(model);
        }

        public string Predict(string cargo, string skill, string nivel, string formacao)
        {
            var input = new TrainingData
            {
                CargoAtual = cargo,
                SkillPrincipal = skill,
                NivelSkill = nivel,
                Formacao = formacao
            };

            return _engine.Predict(input).PredictedArea;
        }
    }
}
