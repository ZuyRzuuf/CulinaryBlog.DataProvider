drop database if exists culinary_blog_test;
create database if not exists culinary_blog_test;
use culinary_blog_test;

create table Recipe
(
    uuid    varchar(36) primary key,
    title   varchar(255) not null
);