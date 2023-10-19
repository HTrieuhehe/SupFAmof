using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<BaseResponseViewModel<NotificationHistoryResponse>> CreateNotification(CreateNotificationHistoryRequest request)
        {
            try
            {
                var noti = _mapper.Map<CreateNotificationHistoryRequest, NotificationHistory>(request);

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
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                Constants.LimitPaging, Constants.DefaultPaging);

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
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                Constants.LimitPaging, Constants.DefaultPaging);

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
    }
}
