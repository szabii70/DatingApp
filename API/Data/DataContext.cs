﻿using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public virtual DbSet<AppUser> Users { get; set; }
    public virtual DbSet<UserLike> Likes { get; set; }
    public virtual DbSet<Message> Messages { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
            .HasKey(k => new {k.SourceUserId, k.TargetUserId});

        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(l => l.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(l => l.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
