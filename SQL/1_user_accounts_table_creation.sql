create table UserAccounts (
	Id varchar(900) not null primary key,
	UserName varchar(max) not null,
	Email varchar(max) not null,
	Password varchar(max) not null,
	IsEmailVerified bit not null,
	IsActive bit not null,
	CreationDate datetime not null
);

insert into MigrationHistory(ScriptName) values('1_user_accounts_table_creation');