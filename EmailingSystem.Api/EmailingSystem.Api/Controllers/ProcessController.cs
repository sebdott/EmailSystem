using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using EmailingSystem.Common.Providers.Interface;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Api.Managers;
using System.Collections.Generic;

namespace EmailingSystem.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProcessController : Controller
    {
        ILogProvider _logProvider;
        private IProcessManager _processManager;
        private IOptions<KafkaSettings> _kafkaSettings;

        public ProcessController(
            ILogProvider logProvider,
            IOptions<KafkaSettings> kafkaSettings,
            IProcessManager processManager)
        {
            _logProvider = logProvider;
            _kafkaSettings = kafkaSettings;
            _processManager = processManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailLog(int currentPage, int pageSize)
        {
            try
            {
                return Ok(await _processManager.GetEmailLog(currentPage, pageSize));
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("GetEmailLog", "Process Fail", ex);
                return NotFound(new DisplayView<EmailLog>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserList(int currentPage, int pageSize)
        {
            try
            {
                return Ok(await _processManager.GetUserList(currentPage, pageSize));
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("GetUserList", "Process Fail", ex);
                return NotFound(new DisplayView<User>());
            }
        }
        [HttpPost]
        public async Task<IActionResult> InitiateSetStatusProcess()
        {
            try
            {
                var isSuccess = await _processManager.InitiateSetStatusProcess();
                return Ok(new { isSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("InitiateProcessStatus", "Process Fail", ex);
                return NotFound(new { isSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InitiateSendEmailProcess()
        {
            try
            {
                var isSuccess = await _processManager.InitiateSendEmailProcess();
                return Ok(new { isSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("InitiateProcessStatus", "Process Fail", ex);
                return NotFound(new { isSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetTestData(int? records)
        {
            try
            {
                var isSuccess = await _processManager.ResetTestData(records != null ? records.Value : 50);
                return Ok(new { isSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("InitiateProcessStatus", "Process Fail", ex);
                return NotFound(new { isSuccess = false });
            }
        }
    }
}
