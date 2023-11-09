using AutoMapper;
using System.Linq;
using Service.Commons;
using Expo.Server.Client;
using Expo.Server.Models;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.Service
{
    public class ExpoTokenService : IExpoTokenService
    {
        private readonly IFirebaseMessagingService _fmService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExpoTokenService(IUnitOfWork unitOfWork, IMapper mapper, IFirebaseMessagingService fmService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fmService = fmService;
        }

        public void AddFcmToken(string fcmToken, int accountId)
        {
            try
            {
                var fcm = _unitOfWork.Repository<Fcmtoken>().GetAll()
                    .FirstOrDefault(x => x.AccountId == accountId);

                if (fcm == null)
                {
                    _fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);

                    var newtoken = new Fcmtoken()
                    {
                        Token = fcmToken,
                        AccountId = accountId,
                        CreateAt = DateTime.UtcNow
                    };
                    _unitOfWork.Repository<Fcmtoken>().Insert(newtoken);
                    _unitOfWork.Commit();
                }
                else if (!fcm.Token.Equals(fcmToken))
                {
                    //_fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);
                    fcm.Token = fcmToken;
                    fcm.AccountId = accountId;
                    fcm.UpdateAt = DateTime.UtcNow;

                    _unitOfWork.Repository<Fcmtoken>().UpdateDetached(fcm);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                var fcm = _unitOfWork.Repository<Fcmtoken>().GetAll()
                    .FirstOrDefault(x => x.AccountId == accountId);

                if (fcm == null)
                {
                    _fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);

                    var newtoken = new Fcmtoken()
                    {
                        Token = fcmToken,
                        AccountId = accountId,
                        CreateAt = DateTime.UtcNow
                    };
                    _unitOfWork.Repository<Fcmtoken>().Insert(newtoken);
                    _unitOfWork.Commit();
                }
                else if (!fcm.Token.Equals(fcmToken))
                {
                    //_fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);
                    fcm.Token = fcmToken;
                    fcm.AccountId = accountId;
                    fcm.UpdateAt = DateTime.UtcNow;

                    _unitOfWork.Repository<Fcmtoken>().UpdateDetached(fcm);
                    _unitOfWork.Commit();
                }
            }
        }

        public void AddStaffFcmToken(string fcmToken, int staffId)
        {
            try
            {
                var fcm = _unitOfWork.Repository<Fcmtoken>().GetAll()
                    .FirstOrDefault(x => x.StaffId == staffId);

                if (fcm == null)
                {
                    _fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);

                    var newtoken = new Fcmtoken()
                    {
                        Token = fcmToken,
                        StaffId = staffId,
                        CreateAt = DateTime.UtcNow
                    };
                    _unitOfWork.Repository<Fcmtoken>().Insert(newtoken);
                    _unitOfWork.Commit();
                }
                else if (!fcm.Token.Equals(fcmToken))
                {
                    _fmService.Subcribe(new List<string>() { fcmToken }, Constants.NOTIFICATION_TOPIC);

                    fcm.StaffId = staffId;
                    fcm.UpdateAt = DateTime.UtcNow;

                    _unitOfWork.Repository<Fcmtoken>().Update(fcm, fcm.Id);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int RemoveFcmTokens(ICollection<string> fcmTokens)
        {
            try
            {
                var tokens = _unitOfWork.Repository<Fcmtoken>().GetAll();

                if (tokens == null)
                    return 0;

                var cusTokens = tokens.Where(x => x.AccountId != null).Select(x => x.Token).ToList();
                if (cusTokens != null && cusTokens.Count > 0)
                    _fmService.Unsubcribe(cusTokens, Constants.NOTIFICATION_TOPIC);

                _unitOfWork.Repository<Fcmtoken>().DeleteRange(tokens);
                _unitOfWork.Commit();

                return tokens.Count();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SubscribeAll(int accountId)
        {
            try
            {
                var tokensMapping = _unitOfWork.Repository<Fcmtoken>().GetAll().Where(x => x.AccountId.Equals(accountId));
                if (tokensMapping != null)
                {
                    var tokens = tokensMapping.Select(x => x.Token).ToList();
                    _fmService.Subcribe(tokens, Constants.NOTIFICATION_TOPIC);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UnsubscribeAll(int accountId)
        {
            try
            {
                var tokensMapping = _unitOfWork.Repository<Fcmtoken>().GetAll().Where(x => x.AccountId.Equals(accountId));
                if (tokensMapping != null)
                {
                    var tokens = tokensMapping.Select(x => x.Token).ToList();
                    _fmService.Unsubcribe(tokens, Constants.NOTIFICATION_TOPIC);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ValidToken(string fcmToken)
        {
            return await _fmService.ValidToken(fcmToken);
        }

        #region ExpoToken

        public void AddAdminExpoToken(string expoToken, int adminId)
        {
            try
            {
                
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public void AddExpoToken(string expoToken, int accountId)
        {
            try
            {
                var expo = _unitOfWork.Repository<ExpoPushToken>().Find(x => x.AccountId == accountId && x.Token.Equals(expoToken));

                if (expo == null)
                {
                    var newtoken = new ExpoPushToken()
                    {
                        Token = expoToken,
                        AccountId = accountId,
                        CreateAt = Ultils.GetCurrentDatetime(),
                    };
                    _unitOfWork.Repository<ExpoPushToken>().Insert(newtoken);
                    _unitOfWork.Commit();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<int> RemoveExpoTokens(ICollection<string> expoTokens, int accountId, int status)
        {
            try
            {
                switch (status)
                {
                    case 1:
                        //remove only token (Casual Logout)
                        var tokens = _unitOfWork.Repository<ExpoPushToken>().GetAll().Where(x => x.Token.Equals(expoTokens) && x.AccountId == accountId || x.AdminId == accountId);

                        if (tokens == null)
                            return 0;

                        _unitOfWork.Repository<ExpoPushToken>().DeleteRange(tokens);
                        _unitOfWork.Commit();

                        return tokens.Count();

                    case 2:
                        //remove all token (When client disable there account
                        var allTokens = _unitOfWork.Repository<ExpoPushToken>().GetAll().Where(x => x.AccountId == accountId || x.AdminId == accountId);

                        if (allTokens == null)
                            return 0;

                        _unitOfWork.Repository<ExpoPushToken>().DeleteRange(allTokens);
                        _unitOfWork.Commit();

                        return allTokens.Count();

                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ValidExpoToken(string expoToken, int accountId)
        {
            try
            {
                var expocheck =  await _unitOfWork.Repository<ExpoPushToken>()
                                        .FindAsync(x => x.Token.Equals(expoToken) && x.AccountId == accountId || x.AdminId == accountId);

                if (expoToken == null)
                {
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
    
}
