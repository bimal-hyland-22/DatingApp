using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public MessageRepository(DataContext dataContext,IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }
        public void AddMessage(Message message)
        {
           dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
          return await dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParam messageParam)
        {
            var query=dataContext.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();
            query=messageParam.Container switch
            {
                "Inbox" =>query.Where(u=>u.RecipentUserName==messageParam.UserName && u.RecipentDeleted==false),
                "Outbox" => query.Where(u=>u.SenderUserName==messageParam.UserName && u.SenderDeleted==false),
                _=>query.Where(u=>u.RecipentUserName==messageParam.UserName && u.DateRead==null && u.RecipentDeleted==false)
            };
            var message=query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.ToPagedList(message,messageParam.PageNumber,messageParam.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName,string recipientUserName)
        {
           var message=await dataContext.Messages
           .Include(u=>u.Sender).ThenInclude(p=>p.Photos)
           .Include(u=>u.Sender).ThenInclude(p=>p.Photos)
           .Where(m=>m.RecipentUserName==currentUserName && m.RecipentDeleted==false && m.SenderUserName==recipientUserName || m.RecipentUserName==recipientUserName && m.SenderDeleted==false && m.SenderUserName==currentUserName)
           .OrderBy(m=>m.MessageSent).ToListAsync();
           var unread=message.Where(m=>m.DateRead==null && m.RecipentUserName==currentUserName).ToList();

           if(unread.Any()){
            foreach (var item in unread)
            {
                item.DateRead=DateTime.UtcNow;
            }
            await dataContext.SaveChangesAsync();
           }

           return mapper.Map<IEnumerable<MessageDto>>(message);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await dataContext.SaveChangesAsync()>0;
        }
    }
}