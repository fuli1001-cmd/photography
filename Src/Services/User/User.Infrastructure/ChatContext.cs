using Arise.DDD.Domain.SeedWork;
using Arise.DDD.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Photography.Services.User.Domain.BackwardCompatibility.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Photography.Services.User.Infrastructure
{
    public class ChatContext : BaseContext
    {
        public DbSet<PSR_ARS_Chat> PSR_ARS_Chat { get; set; }

        public DbSet<PSR_ARS_MessageOffline> PSR_ARS_MessageOffline { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options, IMediator mediator) : base(options, mediator) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PSR_ARS_Chat>().HasKey(p => p.IMARSC_Id);
            modelBuilder.Entity<PSR_ARS_MessageOffline>().HasKey(p => p.IMARSMNRR_Uid);
        }
    }
}
