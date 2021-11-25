create table users (
	discordId bigint unsigned not null unique primary key, 
    isDev bool not null Default false)
