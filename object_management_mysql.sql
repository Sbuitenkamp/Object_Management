CREATE TABLE `Object` (
	`object_number` INT(11) NOT NULL,
	`object_type_id` INT(11) NOT NULL,
	`loaned_out` BOOLEAN NOT NULL DEFAULT false,
	`in_service` BOOLEAN NOT NULL DEFAULT false,
	`price` FLOAT NOT NULL,
	`bulk_sale` varchar(255),
	PRIMARY KEY (`object_number`)
);

CREATE TABLE `Customer` (
	`id` INT(11) NOT NULL AUTO_INCREMENT,
	`name` varchar(255) NOT NULL,
	`telephone` INT(11) NOT NULL,
	`email` varchar(255) NOT NULL,
	`adres` varchar(255),
	PRIMARY KEY (`id`)
);

CREATE TABLE `Reservation` (
	`reservation_number` INT(11) NOT NULL AUTO_INCREMENT,
	`customer_id` INT(11) NOT NULL,
	`start_date` DATE NOT NULL,
	`return_date` DATE NOT NULL,
	`residence` varchar(255) NOT NULL,
	`comment` varchar(255) NOT NULL,
	`sale` FLOAT,
	`payment_method` varchar(255) NOT NULL,
	PRIMARY KEY (`reservation_number`)
);

CREATE TABLE `is_reserved_at` (
	`object_number` INT(11) NOT NULL,
	`reservation_number` INT(11) NOT NULL
);

CREATE TABLE `Sale` (
	`id` INT(11) NOT NULL AUTO_INCREMENT,
	`days_to_rent` INT(11) NOT NULL,
	`days_to_pay` INT(11) NOT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `ObjectType` (
	`id` INT(11) NOT NULL AUTO_INCREMENT,
	`description` VARCHAR(255) NOT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `is_applied_to` (
	`sale_id` INT(11) NOT NULL,
	`object_type_id` INT(11) NOT NULL
);

ALTER TABLE `Object` ADD CONSTRAINT `Object_fk0` FOREIGN KEY (`object_type_id`) REFERENCES `ObjectType`(`id`);

ALTER TABLE `Reservation` ADD CONSTRAINT `Reservation_fk0` FOREIGN KEY (`customer_id`) REFERENCES `Customer`(`id`);

ALTER TABLE `is_reserved_at` ADD CONSTRAINT `is_reserved_at_fk0` FOREIGN KEY (`object_number`) REFERENCES `Object`(`object_number`);

ALTER TABLE `is_reserved_at` ADD CONSTRAINT `is_reserved_at_fk1` FOREIGN KEY (`reservation_number`) REFERENCES `Reservation`(`reservation_number`);

ALTER TABLE `is_applied_to` ADD CONSTRAINT `is_applied_to_fk0` FOREIGN KEY (`sale_id`) REFERENCES `Sale`(`id`);

ALTER TABLE `is_applied_to` ADD CONSTRAINT `is_applied_to_fk1` FOREIGN KEY (`object_type_id`) REFERENCES `ObjectType`(`id`);








