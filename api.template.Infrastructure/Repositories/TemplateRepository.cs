using Dapper;
using Microsoft.Data.SqlClient;
using Api.Template.ApplicationCore.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Template.ApplicationCore.Dto;
using System.Text.RegularExpressions;
using Api.Template.ApplicationCore.Interfaces.Repositories;

namespace Api.Template.Infrastructure.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        public async Task<ApplicationCore.Entities.Template> GetTemplateByIdAsync(int templateId)
        {
            var @params = new DynamicParameters();

            @params.Add("@TemplateId", templateId, DbType.Int32);

            var sql = @"
			SELECT
			TemplateId AS TemplateId,
            Name AS Name,
            Content AS Content
			FROM [dbo].[Template] (NOLOCK)
			WHERE TemplateId = @TemplateId;";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<ApplicationCore.Entities.Template>(sql, @params);

                if (result.AsList().Count == 0)
                    return null;

                return result.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<ApplicationCore.Entities.Template>> GetTemplateFullDetailsByFilterAsync(TemplateSearchFilterDto filter)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                string sQuery = $@"
                SELECT
			    TemplateId AS TemplateId,
                Name AS Name,
                Content AS Content

			    FROM [dbo].[Template] (NOLOCK)
                WHERE 1 = 1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(filter.KeyWord))
                {
                    parameters.Add("@Name", filter.KeyWord, DbType.AnsiString, ParameterDirection.Input);
                    sQuery += " AND ( [Name] LIKE '%' + @Name + '%' ";

                    if (Regex.Replace(filter.KeyWord, @"[^\d]", "").Length > 0)
                    {
                        parameters.Add("@TemplateId", Regex.Replace(filter.KeyWord, @"[^\d]", ""), DbType.Int64, ParameterDirection.Input);
                        sQuery += " OR s.[TemplateId] = @TemplateId ";
                    }
                    sQuery += " ) ";
                }

                parameters.Add("@Page", filter.PageNumber, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Row", filter.RowsPerPage, DbType.Int32, ParameterDirection.Input);
                sQuery += @" ORDER BY [Name] 
                OFFSET @Page*@Row ROWS
                FETCH NEXT @Row ROWS ONLY;";

                connection.Open();

                return await connection.QueryAsync<ApplicationCore.Entities.Template>(sQuery, parameters);
            }
        }

        public async Task<int> InsertTemplateAsync(ApplicationCore.Entities.Template Template)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Name", Template.Name, DbType.AnsiString, ParameterDirection.Input);
                parameters.Add("@Content", Template.Content, DbType.AnsiString, ParameterDirection.Input);

                var sql = @"
						INSERT INTO [dbo].[Template]
                              ([Name]
                              ,[Content])
                        OUTPUT INSERTED.[TemplateId]
                        VALUES
                              (@Name
                              ,@Content);";

                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
            }
        }

        public async Task UpdateTemplateAsync(ApplicationCore.Entities.Template Template)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TemplateId", Template.TemplateId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Name", Template.Name, DbType.AnsiString, ParameterDirection.Input);
                parameters.Add("@Content", Template.Content, DbType.AnsiString, ParameterDirection.Input);

                var sql = @"
						UPDATE [dbo].[Template]
                        SET [Name] = @Name
                             ,[Content] = @Content
                        WHERE [TemplateId] = @TemplateId;";

                connection.Open();
                await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}