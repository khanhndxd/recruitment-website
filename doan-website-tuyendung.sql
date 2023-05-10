create database Doan_WebsiteTuyenDung
use Doan_WebsiteTuyenDung

create table Job_Seeker(
	JS_id varchar(30) primary key not null,
	JS_name nvarchar(30) not null,
	JS_email varchar(30) not null,
	JS_password varchar(30) not null,
	JS_phone varchar(11),
	JS_address nvarchar(100),
	JS_skills nvarchar(100),
	JS_expectedSalary money,
	JS_image nvarchar(MAX)
)

create table Employer(
	E_id varchar(30) primary key not null,
	E_name nvarchar(30) not null,
	E_email nvarchar(30) not null,
	E_password varchar(30) not null,
	E_contactPerson nvarchar(30),
	E_phone varchar(11),
	E_address nvarchar(100),
	E_about nvarchar(100),
	E_image nvarchar(MAX)
)


create table Job(
	J_id varchar(30) primary key not null,
	J_title nvarchar(100) not null,
	J_description nvarchar(MAX) not null,
	J_requiredSkills nvarchar(MAX) not null,
	J_salary money not null,
	J_postDate date default(GETDATE()) not null,
	J_expirationDate date not null,
	J_status int,
	J_Location nvarchar(100),
	J_Type nvarchar(10) not null,
	E_id varchar(30) not null CONSTRAINT FK_Job 
			FOREIGN KEY (E_id) REFERENCES Employer(E_id)
)

drop table Employer
drop table Job_Seeker
drop table Resume

drop table Job
drop table Application
drop table Intermediate_table

create table Application(
	A_id varchar(30) primary key not null,
	A_appliedDate date default(GETDATE()) not null,
	A_status int not null,
	A_feedBack nvarchar(100),
	J_id varchar(30) not null CONSTRAINT FK_App 
			FOREIGN KEY (J_id) REFERENCES Job(J_id)
)

create table Resume(
	R_id varchar(30) primary key not null,
	R_name nvarchar(30) not null,
	R_updateDate date default(GETDATE()) not null,
	R_content nvarchar(100) not null,
	JS_id varchar(30) not null 
	CONSTRAINT FK_Resume 
			FOREIGN KEY (JS_id) REFERENCES Job_Seeker(JS_id)
)

create table Intermediate_table(
	R_id varchar(30) not null,
	A_id varchar(30) not null,
	primary key(R_id,A_id),
	CONSTRAINT FK_Intermediate 
			FOREIGN KEY (R_id) REFERENCES Resume(R_id),
	CONSTRAINT FK_Intermediate1 
			FOREIGN KEY (A_id) REFERENCES Application(A_id),
)