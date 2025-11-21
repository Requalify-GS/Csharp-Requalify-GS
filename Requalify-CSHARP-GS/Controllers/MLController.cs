using Microsoft.AspNetCore.Mvc;
using Requalify.ML;

namespace Requalify.Controllers
{
    /// <summary>
    /// Controller responsible for integrating the Machine Learning (ML.NET) model
    /// to generate predictions about the recommended professional area for a user.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "ml")]
    [Route("api/ml")]
    public class MLController(InterestPredictionService ml) : ControllerBase
    {
        private readonly InterestPredictionService _ml = ml;

        /// <summary>
        /// Predicts the recommended professional area based on the provided input data.
        /// </summary>
        /// <remarks>
        /// This endpoint uses an ML.NET model trained to analyze information such as:
        /// current job role, main skill, skill level, and academic background, returning
        /// the most likely matching professional area.
        /// </remarks>
        /// <param name="request">Input data used by the ML model.</param>
        /// <returns>The professional area recommended by the predictive model.</returns>
        [HttpPost("predict-interest")]
        public IActionResult Predict(PredictInterestRequest request)
        {
            var result = _ml.Predict(
                request.CurrentRole,
                request.MainSkill,
                request.SkillLevel,
                request.Education
            );

            return Ok(new { recommendedArea = result });
        }
    }

    /// <summary>
    /// Represents the input data required for the professional area prediction model.
    /// </summary>
    public class PredictInterestRequest
    {
        /// <summary>
        /// User's current job role.
        /// </summary>
        public string CurrentRole { get; set; }

        /// <summary>
        /// User's main skill.
        /// </summary>
        public string MainSkill { get; set; }

        /// <summary>
        /// Level of the main skill (e.g., Beginner, Intermediate, Advanced).
        /// </summary>
        public string SkillLevel { get; set; }

        /// <summary>
        /// User's academic background.
        /// </summary>
        public string Education { get; set; }
    }
}
