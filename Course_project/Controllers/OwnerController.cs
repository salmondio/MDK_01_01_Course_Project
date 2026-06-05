using Course_project_wpf.Helpers;
using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Course_project_wpf.Controllers
{
    public class OwnerController
    {
        // Действия над оценками
        public Evaluation? AddEvaluation(Evaluation evaluation)
        {
            Evaluation newEvaluation = new Evaluation();

            return newEvaluation;
        }

        public async Task<Evaluation?> UpdateEvaluation(Evaluation evaluation)
        {
            Evaluation updatedEvaluation = new Evaluation();

            var response = await ApiClient.PutAsync("api/Evaluation/OwnerUpdate", evaluation);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                updatedEvaluation = JsonSerializer.Deserialize<Evaluation>(responseBody);
            }
            else
                MessageBox.Show("Ошибка: Не удалось обновить оценку: " + response.RequestMessage + " код ошибки: " + response.StatusCode, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            return updatedEvaluation;
        }
    }
}
