using System.Collections.Generic;

namespace Requalify.ML
{
    public static class TrainingDataset
    {
        public static List<TrainingData> Data = new List<TrainingData>
        {
            new TrainingData { CargoAtual="Analista de Dados", SkillPrincipal="SQL", NivelSkill="Intermediário", Formacao="TI", AreaInteresse="Cientista de Dados" },
            new TrainingData { CargoAtual="Desenvolvedor Junior", SkillPrincipal="C#", NivelSkill="Básico", Formacao="TI", AreaInteresse="Desenvolvedor Backend" },
            new TrainingData { CargoAtual="Designer", SkillPrincipal="UI Design", NivelSkill="Intermediário", Formacao="Design", AreaInteresse="UX/UI" },
            new TrainingData { CargoAtual="Analista de Suporte", SkillPrincipal="Redes", NivelSkill="Intermediário", Formacao="TI", AreaInteresse="Infraestrutura" },
            new TrainingData { CargoAtual="Professor", SkillPrincipal="Pedagogia", NivelSkill="Avançado", Formacao="Humanas", AreaInteresse="Docência" },
        };
    }
}
