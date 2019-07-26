/*
 Navicat Premium Data Transfer

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 80016
 Source Host           : localhost:3306
 Source Schema         : test

 Target Server Type    : MySQL
 Target Server Version : 80016
 File Encoding         : 65001

 Date: 26/07/2019 18:29:24
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for school
-- ----------------------------
DROP TABLE IF EXISTS `school`;
CREATE TABLE `school`  (
  `id` int(11) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of school
-- ----------------------------
INSERT INTO `school` VALUES (1, '清华', NULL);

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `grade` int(11) NULL DEFAULT NULL,
  `age` int(11) NULL DEFAULT NULL,
  `school_id` int(11) NULL DEFAULT NULL,
  `version` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `create_time` datetime(0) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 23 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of student
-- ----------------------------
INSERT INTO `student` VALUES (14, 'jbck', 0, 5, 1, '12', '2019-07-25 16:07:00');
INSERT INTO `student` VALUES (15, 'tom', 0, 20, NULL, '1', '2019-07-25 16:07:04');
INSERT INTO `student` VALUES (17, 'sub', 3, 60, 1, 'r', '2019-07-25 17:19:24');
INSERT INTO `student` VALUES (18, 'jack', 1, 20, NULL, 'f', '2019-07-25 17:19:24');
INSERT INTO `student` VALUES (19, 'bob', 4, 80, NULL, 'a', '2019-07-25 17:19:24');
INSERT INTO `student` VALUES (22, 'jar', 4, 90, NULL, 'f', '2019-07-25 17:19:24');

SET FOREIGN_KEY_CHECKS = 1;
