using GitHubRepoAdmin.Domain;
using GitHubRepoAdmin.Domain.Interfaces;
using GitHubRepoAdmin.Infra.CrossCutting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GitHubRepoAdmin.Infra.ApiGitHub
{
    public class GitHubApi : IGitHubApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public GitHubApi(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["GitHubApi:BaseUrl"] ?? "https://api.github.com";
        }

        private async Task<ActionResult<TModel>> FetchFromGitHubApi<TModel>(string endpoint, string errorMessage) where TModel : class, new()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{endpoint}");
                request.Headers.Add("User-Agent", "EvoHub");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ActionResult<TModel>
                    {
                        IsValid = false,
                        Message = errorMessage,
                        Result = new TModel()
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TModel>(content) ?? new TModel();

                return new ActionResult<TModel>
                {
                    IsValid = true,
                    Message = Constants.SucessRepositoriesRetrieved,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging library like Serilog or NLog)
                return new ActionResult<TModel>
                {
                    IsValid = false,
                    Message = $"An error occurred: {ex.Message}",
                    Result = new TModel()
                };
            }
        }

        public async Task<ActionResult<List<GitHubRepository>>> GetRepositoriesByOwner(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                return new ActionResult<List<GitHubRepository>>
                {
                    IsValid = false,
                    Message = "Owner cannot be null or empty.",
                    Result = new List<GitHubRepository>()
                };
            }

            string endpoint = $"/users/{owner}/repos";
            string errorMessage = Constants.ErrorCouldntRetrieveUserRepositories;

            return await FetchFromGitHubApi<List<GitHubRepository>>(endpoint, errorMessage);
        }

        public async Task<ActionResult<RepositoryModel>> GetRepositoriesByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ActionResult<RepositoryModel>
                {
                    IsValid = false,
                    Message = "Repository name cannot be null or empty.",
                    Result = new RepositoryModel()
                };
            }

            string endpoint = $"/search/repositories?q={name}";
            string errorMessage = Constants.ErrorCouldntRetrieveRepositoriesRelatedToName;

            return await FetchFromGitHubApi<RepositoryModel>(endpoint, errorMessage);
        }
    }
}
