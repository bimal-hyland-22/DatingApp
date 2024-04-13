using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
   
    public class MessagesController : BaseController
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(ILogger<MessagesController> logger,IUserRepository userRepository, IMessageRepository messageRepository ,IMapper mapper)
        {
            this.mapper = mapper;
            _logger = logger;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
        }
        [HttpPost()]
        public async Task<ActionResult<MessageDto>> CreateMessageDto(CreateMessageDto createMessageDto){
                var username=User.GetUserName();
                _logger.LogInformation(username);
                 if(username==createMessageDto.RecipientUserName.ToLower())
                     return BadRequest("You cannot send message to yourself");
                var sender=await userRepository.GetUserByNameAsync(username);
                var recipent=await userRepository.GetUserByNameAsync(createMessageDto.RecipientUserName);
                if(recipent==null)
                return BadRequest("Not recipent user");
                var message=new Message(){
                    Content = createMessageDto.Content,
                    Sender=sender,
                    Recipent=recipent,
                    SenderUserName=sender.UserName,
                    RecipentUserName=recipent.UserName,
                };

                messageRepository.AddMessage(message);

                if(await messageRepository.SaveAllAsync())
                 return Ok(mapper.Map<MessageDto>(message));
                return BadRequest("Message failed to send" );
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParam messageParam)
        {
            messageParam.UserName=User.GetUserName();
            var message=await messageRepository.GetMessagesForUser(messageParam);
            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage,message.PageSize,message.TotalCount,message.TotalPages));
            return message;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
            var currentUserName=User.GetUserName();
            return Ok(await messageRepository.GetMessageThread(currentUserName,username));
        }
            
       [HttpDelete("{id}")]
       public async Task<ActionResult> DeleteMessage(int id){
            var username=User.GetUserName();
            var message=await messageRepository.GetMessage(id);
            if(message.RecipentUserName != username && message.SenderUserName!=username)
             return Unauthorized();

             if(message.SenderUserName==username)
                 message.SenderDeleted=true;
             if(message.RecipentUserName==username)
               message.RecipentDeleted=true;
             if(message.SenderDeleted && message.RecipentDeleted)
              {
                messageRepository.DeleteMessage(message);
              }     
              if(await messageRepository.SaveAllAsync())
                return Ok();
             return BadRequest("Problem in deleting messages");

       }
       
    }
}