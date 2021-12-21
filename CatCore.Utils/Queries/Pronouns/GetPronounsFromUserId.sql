select 
	pronouns.id, 
	pronouns.subject, 
    pronouns.object, 
    pronouns.possessive, 
    pronouns.reflexive 
from 
	pronouns,
    userpronouns
where 
	userpronouns.pronounId=pronouns.id and
    userpronouns.userId=@userId;