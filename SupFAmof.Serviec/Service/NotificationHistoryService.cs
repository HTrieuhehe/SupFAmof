using AutoMapper;
using AutoMapper.QueryableExtensions;
using Expo.Server.Client;
using Expo.Server.Models;
using LAK.Sdk.Core.Utilities;
using Org.BouncyCastle.Cms;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.Service.Service
{
    public class NotificationHistoryService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<BaseResponseViewModel<NotificationHistoryResponse>> CreateNotification(CreateNotificationHistoryRequest request)
        {
            try
            {
                var noti = _mapper.Map<CreateNotificationHistoryRequest, NotificationHistory>(request);

                noti.Status = (int)NotificationStatusEnum.Sent;
                noti.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<NotificationHistory>().InsertAsync(noti);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<NotificationHistoryResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<NotificationHistoryResponse>(noti)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<NotificationHistoryResponse>> GetNotificationById(int recipientId, PagingRequest paging)
        {
            try
            {
                var listNoti = _unitOfWork.Repository<NotificationHistory>().GetAll()
                                                .ProjectTo<NotificationHistoryResponse>(_mapper.ConfigurationProvider)
                                                .Where(x => x.RecipientId == recipientId)
                                                .OrderByDescending(x => x.CreateAt)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<NotificationHistoryResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = listNoti.Item1
                    },
                    Data = listNoti.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<NotificationHistoryResponse>> GetNotifications(NotificationHistoryResponse filter, PagingRequest paging)
        {
            try
            {
                var listNoti = _unitOfWork.Repository<NotificationHistory>().GetAll()
                                                .ProjectTo<NotificationHistoryResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<NotificationHistoryResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = listNoti.Item1
                    },
                    Data = listNoti.Item2.ToList(),
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<PushTicketResponse> PushNotification(PushNotificationRequest request)
        {
            try
            {
                //tạo các notification cho từng recipient để lưu database
                //các accountId đã được validate trước khi gọi Push noti bảo đảm rằng các account disable hoặc bị banned sẽ không được nhận thông báo
                foreach (var recipient in request.Ids)
                {
                    CreateNotificationHistoryRequest notificateRequest = new CreateNotificationHistoryRequest()
                    {
                        RecipientId = recipient,
                        Title = request.Title,
                        Text = request.Body,
                        NotificationType = request.NotificationsType 
                    };

                    var notificationHistory = _mapper.Map<CreateNotificationHistoryRequest, NotificationHistory>(notificateRequest);
                    notificationHistory.Status = (int)NotificationStatusEnum.Sent;

                    //saving in db context but not commit
                    await _unitOfWork.Repository<NotificationHistory>().InsertAsync(notificationHistory);
                }

                var tokens = _unitOfWork.Repository<ExpoPushToken>()
                                           .GetAll()
                                           .Where(x => request.Ids.Contains((int)x.AccountId) || request.Ids.Contains((int)x.AdminId))
                                           .Select(y => y.Token.Trim())
                                           .ToList();
                List<string> tokenList = Ultils.TurnToExpoPushToken(tokens);

                var expoSDKClient = new PushApiClient();
                var pushTicketReq = new PushTicketRequest()
                {
                    PushTo = tokenList,
                    PushBadgeCount = 7,
                    PushBody = request.Body.Trim(),
                    PushTitle = request.Title.Trim()
                };
                var result = await expoSDKClient.PushSendAsync(pushTicketReq);
                if (result?.PushTicketErrors?.Count > 0)
                {
                    foreach (var error in result.PushTicketErrors)
                    {
                        Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                    }
                }

                //commit these data to database
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
