CREATE SCHEMA IF NOT EXISTS `bibliotech`;
USE `bibliotech` ;

CREATE TABLE IF NOT EXISTS `bibliotech`.`status` (
  `id_status` INT NOT NULL,
  `description` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`id_status`)
  );

CREATE TABLE IF NOT EXISTS `bibliotech`.`address` (
  `id_address` INT NOT NULL AUTO_INCREMENT,
  `city` VARCHAR(45) NOT NULL,
  `neighborhood` VARCHAR(45) NOT NULL,
  `street` VARCHAR(100) NOT NULL,
  `number` VARCHAR(5) NOT NULL,
  `complement` VARCHAR(45) NULL DEFAULT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_address`),
  INDEX `status` (`status` ASC),
  CONSTRAINT `address_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`author` (
  `id_author` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_author`),
  INDEX `status` (`status` ASC),
  CONSTRAINT `author_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`book` (
  `id_book` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(200) NOT NULL,
  `subtitle` VARCHAR(200) NOT NULL,
  `publishing_company` VARCHAR(50) NOT NULL,
  `gender` VARCHAR(50) NULL DEFAULT NULL,
  `edition` VARCHAR(50) NULL DEFAULT NULL,
  `pages` INT NULL DEFAULT NULL,
  `year_publication` INT NULL DEFAULT NULL,
  `language` VARCHAR(50) NULL DEFAULT NULL,
  `volume` VARCHAR(50) NULL DEFAULT NULL,
  `collection` VARCHAR(50) NULL DEFAULT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_book`),
  INDEX `status` (`status` ASC),
  CONSTRAINT `book_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`book_has_author` (
  `id_book` INT NOT NULL,
  `id_author` INT NOT NULL,
  PRIMARY KEY (`id_book`, `id_author`),
  INDEX `id_author` (`id_author` ASC),
  CONSTRAINT `book_has_author_ibfk_1`
    FOREIGN KEY (`id_book`)
    REFERENCES `bibliotech`.`book` (`id_book`),
  CONSTRAINT `book_has_author_ibfk_2`
    FOREIGN KEY (`id_author`)
    REFERENCES `bibliotech`.`author` (`id_author`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`branch` (
  `id_branch` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL,
  `id_address` INT NULL DEFAULT NULL,
  `telephone` BIGINT NULL DEFAULT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_branch`),
  INDEX `status` (`status` ASC),
  INDEX `id_address` (`id_address` ASC),
  CONSTRAINT `branch_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`),
  CONSTRAINT `branch_ibfk_2`
    FOREIGN KEY (`id_address`)
    REFERENCES `bibliotech`.`address` (`id_address`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`exemplary` (
  `id_exemplary` INT NOT NULL AUTO_INCREMENT,
  `id_book` INT NOT NULL,
  `id_branch` INT NOT NULL,
  `id_index` INT NOT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_exemplary`),
  INDEX `id_book` (`id_book` ASC),
  INDEX `id_branch` (`id_branch` ASC),
  INDEX `status` (`status` ASC),
  CONSTRAINT `exemplary_ibfk_1`
    FOREIGN KEY (`id_book`)
    REFERENCES `bibliotech`.`book` (`id_book`),
  CONSTRAINT `exemplary_ibfk_2`
    FOREIGN KEY (`id_branch`)
    REFERENCES `bibliotech`.`branch` (`id_branch`),
  CONSTRAINT `exemplary_ibfk_3`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`lector` (
  `id_lector` INT NOT NULL AUTO_INCREMENT,
  `id_branch` INT NOT NULL,
  `id_address` INT NULL DEFAULT NULL,
  `user_registration` INT NOT NULL,
  `name` VARCHAR(200) NOT NULL,
  `responsible` VARCHAR(200) NULL DEFAULT NULL,
  `birth_date` DATE NULL DEFAULT NULL,
  `telephone` BIGINT NULL DEFAULT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_lector`),
  INDEX `status` (`status` ASC),
  INDEX `id_branch` (`id_branch` ASC),
  INDEX `id_address` (`id_address` ASC),
  CONSTRAINT `lector_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`),
  CONSTRAINT `lector_ibfk_2`
    FOREIGN KEY (`id_branch`)
    REFERENCES `bibliotech`.`branch` (`id_branch`),
  CONSTRAINT `lector_ibfk_3`
    FOREIGN KEY (`id_address`)
    REFERENCES `bibliotech`.`address` (`id_address`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`type_users` (
  `id_type_user` INT NOT NULL AUTO_INCREMENT,
  `description` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`id_type_user`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`users` (
  `id_user` INT NOT NULL AUTO_INCREMENT,
  `id_type_user` INT NOT NULL,
  `id_branch` INT NOT NULL,
  `name` VARCHAR(200) NOT NULL,
  `user_name` VARCHAR(20) NOT NULL,
  `password` VARBINARY(200) NOT NULL,
  `birth_date` DATE NULL DEFAULT NULL,
  `telephone` BIGINT NULL DEFAULT NULL,
  `id_address` INT NOT NULL,
  `status` INT NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_user`),
  INDEX `status` (`status` ASC),
  INDEX `id_address` (`id_address` ASC),
  INDEX `id_type_user` (`id_type_user` ASC),
  INDEX `id_branch` (`id_branch` ASC),
  CONSTRAINT `users_ibfk_1`
    FOREIGN KEY (`status`)
    REFERENCES `bibliotech`.`status` (`id_status`),
  CONSTRAINT `users_ibfk_2`
    FOREIGN KEY (`id_address`)
    REFERENCES `bibliotech`.`address` (`id_address`),
  CONSTRAINT `users_ibfk_3`
    FOREIGN KEY (`id_type_user`)
    REFERENCES `bibliotech`.`type_users` (`id_type_user`),
  CONSTRAINT `users_ibfk_4`
    FOREIGN KEY (`id_branch`)
    REFERENCES `bibliotech`.`branch` (`id_branch`)
);

CREATE TABLE IF NOT EXISTS `bibliotech`.`lending` (
  `id_lending` INT NOT NULL AUTO_INCREMENT,
  `id_exemplary` INT NOT NULL,
  `id_lector` INT NOT NULL,
  `id_user` INT NOT NULL,
  `loan_date` DATETIME NOT NULL,
  `expected_date` DATETIME NOT NULL,
  `return_date` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id_lending`),
  INDEX `id_lector` (`id_lector` ASC),
  INDEX `id_user` (`id_user` ASC),
  INDEX `id_exemplary` (`id_exemplary` ASC),
  CONSTRAINT `lending_ibfk_1`
    FOREIGN KEY (`id_lector`)
    REFERENCES `bibliotech`.`lector` (`id_lector`),
  CONSTRAINT `lending_ibfk_2`
    FOREIGN KEY (`id_user`)
    REFERENCES `bibliotech`.`users` (`id_user`),
  CONSTRAINT `lending_ibfk_3`
    FOREIGN KEY (`id_exemplary`)
    REFERENCES `bibliotech`.`exemplary` (`id_exemplary`)
);
