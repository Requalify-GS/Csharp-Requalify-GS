using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Model;

namespace Requalify.Mappers
{
    public static class EducationMapper
    {
        public static Education ToEntity(this CreateEducationRequest request)
        {
            return new Education
            {
                Degree = request.Degree,
                Instituion = request.Instituion,
                CompletionDate = request.CompletionDate,
                Certificate = request.Certificate,
                UserId = request.UserId
            };
        }

        public static void UpdateEntity(this UpdateEducationRequest request, Education entity)
        {
            entity.Degree = request.Degree;
            entity.Instituion = request.Instituion;
            entity.CompletionDate = request.CompletionDate;
            entity.Certificate = request.Certificate;
        }

        public static EducationResponse ToResponse(this Education entity)
        {
            return new EducationResponse
            {
                Id = entity.Id,
                Degree = entity.Degree,
                Instituion = entity.Instituion,
                CompletionDate = entity.CompletionDate,
                Certificate = entity.Certificate
            };
        }
    }
}
