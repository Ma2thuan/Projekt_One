﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projekt_1.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Project1DBEntities : DbContext
    {
        public Project1DBEntities()
            : base("name=Project1DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<passbook> passbooks { get; set; }
        public virtual DbSet<SavingsAccountType> SavingsAccountTypes { get; set; }
        public virtual DbSet<SavingsDeposit> SavingsDeposits { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<WithdrawalSlip> WithdrawalSlips { get; set; }
    }
}
