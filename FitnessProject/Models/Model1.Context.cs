﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FitnessProject.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FitnessWebDbEntities : DbContext
    {
        public FitnessWebDbEntities()
            : base("name=FitnessWebDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<appointment> appointments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<gym_classes> gym_classes { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<staff> staffs { get; set; }
        public virtual DbSet<training_process> training_process { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}