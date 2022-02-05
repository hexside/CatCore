﻿// <auto-generated />
using System;
using CatCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CatCore.Client.Migrations
{
    [DbContext(typeof(CatCoreContext))]
    [Migration("20220204223141_Messages")]
    partial class Messages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("CatCore.Data.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AdminMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Footer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("CatCore.Data.Poll", b =>
                {
                    b.Property<int>("PollId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Footer")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Max")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Min")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("PollId");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("CatCore.Data.PollRole", b =>
                {
                    b.Property<int>("PollRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PollId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PollRoleId");

                    b.HasIndex("PollId");

                    b.ToTable("PollRoles");
                });

            modelBuilder.Entity("CatCore.Data.Pronoun", b =>
                {
                    b.Property<int>("PronounId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Object")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PossessiveAdjective")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PossessivePronoun")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Reflexive")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("PronounId");

                    b.HasIndex("UserID");

                    b.ToTable("Pronouns");
                });

            modelBuilder.Entity("CatCore.Data.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("DiscordID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDev")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CatCore.Data.UserMessage", b =>
                {
                    b.Property<int>("UserMessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRead")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserMessageId");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("UserMessages");
                });

            modelBuilder.Entity("CatCore.Data.PollRole", b =>
                {
                    b.HasOne("CatCore.Data.Poll", "Poll")
                        .WithMany("Roles")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Poll");
                });

            modelBuilder.Entity("CatCore.Data.Pronoun", b =>
                {
                    b.HasOne("CatCore.Data.User", null)
                        .WithMany("Pronouns")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("CatCore.Data.UserMessage", b =>
                {
                    b.HasOne("CatCore.Data.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatCore.Data.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CatCore.Data.Poll", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("CatCore.Data.User", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Pronouns");
                });
#pragma warning restore 612, 618
        }
    }
}
