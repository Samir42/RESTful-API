using AutoMapper;
using MediatR;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Models;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Post
{
    public class CreateUserPostCommand : IRequest<UserPostDto>
    {
        private readonly int _userId;
        private readonly UserPostForCreationDto _userPostForCreationDto;

        public CreateUserPostCommand(int userId, UserPostForCreationDto userPostForCreationDto)
        {
            _userId = userId;
            _userPostForCreationDto = userPostForCreationDto;
        }

        internal sealed class CreateUserPostCommandHandler : IRequestHandler<CreateUserPostCommand, UserPostDto>
        {
            private IUserRepository _userRepository;
            private IPostRepository _postRepository;
            private IMapper _mapper;

            public CreateUserPostCommandHandler(IUserRepository userRepository, IPostRepository postRepository,
                IMapper mapper)
            {
                _userRepository = userRepository;
                _postRepository = postRepository;
                _mapper = mapper;
            }

            public async Task<UserPostDto> Handle(CreateUserPostCommand request, CancellationToken cancellationToken)
            {
                //TODO: Refactor this
                //if (!await _userRepository.UserExistsAsync(request._userId))
                //{
                //    throw new KeyNotFoundException($"No user found for {request._userId}");
                //}

                var userPostEntity = _mapper.Map<UserPost>(request._userPostForCreationDto);

                _postRepository.AddUserPost(request._userId, userPostEntity);

                await _postRepository.SaveChangesAsync();

                var userPostDtoToReturn = _mapper.Map<UserPostDto>(userPostEntity);

                return userPostDtoToReturn;
            }
        }
    }
}
