/*
 Navicat Premium Data Transfer

 Source Server         : CartoonProject
 Source Server Type    : MySQL
 Source Server Version : 80021
 Source Host           : localhost:3306
 Source Schema         : quartz

 Target Server Type    : MySQL
 Target Server Version : 80021
 File Encoding         : 65001

 Date: 07/11/2020 00:34:40
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for task
-- ----------------------------
DROP TABLE IF EXISTS `task`;
CREATE TABLE `task`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `TaskName` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '任务名称',
  `Remarks` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '任务备注',
  `CreateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '定时任务创建时间',
  `UpdateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '定时任务修改时间',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '定时任务表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for tasklog
-- ----------------------------
DROP TABLE IF EXISTS `tasklog`;
CREATE TABLE `tasklog`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `TaskId` int(0) NOT NULL COMMENT '定时任务Id',
  `MsgType` int(0) NOT NULL DEFAULT 1 COMMENT '定时任务消息类型，1-正常消息，2-异常消息',
  `Msg` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '定时任务消息',
  `CreateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '定时任务消息创建时间',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_TaskLog_TaskId`(`TaskId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3105 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '定时任务日志表' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
