/*
 Navicat Premium Data Transfer

 Source Server         : localhost_3306
 Source Server Type    : MySQL
 Source Server Version : 100605
 Source Host           : localhost:3308
 Source Schema         : mango

 Target Server Type    : MySQL
 Target Server Version : 100605
 File Encoding         : 65001

 Date: 23/12/2021 17:06:46
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for user_stats
-- ----------------------------
DROP TABLE IF EXISTS `user_stats`;
CREATE TABLE `user_stats`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `respects` int NOT NULL,
  `respects_left_player` int NOT NULL,
  `respects_left_bot` int NOT NULL,
  `moderation_tickets` int NOT NULL,
  `moderation_tickets_abusive` int NOT NULL,
  `moderation_tickets_cooldown` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `moderation_bans` int NOT NULL,
  `moderation_cautions` int NOT NULL,
  `moderation_muted_until` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `timestamp_last_online` varchar(250) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `timestamp_registered` varchar(250) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
