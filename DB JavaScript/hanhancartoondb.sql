/*
 Navicat Premium Data Transfer

 Source Server         : CartoonProject
 Source Server Type    : MySQL
 Source Server Version : 80021
 Source Host           : localhost:3306
 Source Schema         : hanhancartoondb

 Target Server Type    : MySQL
 Target Server Version : 80021
 File Encoding         : 65001

 Date: 07/11/2020 00:32:06
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for hanhancartoon
-- ----------------------------
DROP TABLE IF EXISTS `hanhancartoon`;
CREATE TABLE `hanhancartoon`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `CartoonName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '漫画名',
  `WebPageUrl` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '网页地址',
  `State` int(0) NOT NULL DEFAULT 0 COMMENT '漫画状态：0-连载中，1-已完结',
  `Author` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '漫画作者',
  `WebCoverImgUrl` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '漫画封面图片路径',
  `LocalhostCoverImgUrl` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '本地漫画封面图片路径',
  `EvaluateFraction` decimal(6, 2) NOT NULL DEFAULT 0.00 COMMENT '评分',
  `EvaluateNumber` int(0) NOT NULL DEFAULT 0 COMMENT '评分人数',
  `CollectionNumber` int(0) NOT NULL DEFAULT 0 COMMENT '收藏数',
  `Synopsis` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '简介',
  `CreateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '漫画创建时间（本地）',
  `UpdateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '漫画更新时间',
  `LastSyncTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' ON UPDATE CURRENT_TIMESTAMP(0) COMMENT '最后同步时间',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '汗汗漫画主表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for hanhancartoonauthor
-- ----------------------------
DROP TABLE IF EXISTS `hanhancartoonauthor`;
CREATE TABLE `hanhancartoonauthor`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `HanHanCartoonId` int(0) NOT NULL COMMENT '漫画表Id',
  `HanHanAuthorId` int(0) NOT NULL COMMENT '作者表Id',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_HanHanCartoonAuthor_HanHanCartoonId`(`HanHanCartoonId`) USING BTREE,
  INDEX `IX_HanHanCartoonAuthor_HanHanAuthorId`(`HanHanAuthorId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '汗汗漫画作者关系表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for hanhancartoonchapter
-- ----------------------------
DROP TABLE IF EXISTS `hanhancartoonchapter`;
CREATE TABLE `hanhancartoonchapter`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `HanHanCartoonId` int(0) NOT NULL COMMENT '漫画表Id',
  `ChapterName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '章节名',
  `Sort` int(0) NOT NULL DEFAULT 0 COMMENT '章节排序',
  `CreateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '章节创建时间',
  `UpdateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '章节修改时间',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_HanHanCartoonChapter_HanHanCartoonId`(`HanHanCartoonId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '汗汗漫画章节表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for hanhancartoonpiece
-- ----------------------------
DROP TABLE IF EXISTS `hanhancartoonpiece`;
CREATE TABLE `hanhancartoonpiece`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `HanHanCartoonId` int(0) NOT NULL COMMENT '漫画表Id',
  `HanHanCartoonChapterId` int(0) NOT NULL COMMENT '章节表Id',
  `PieceName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '话名',
  `Sort` int(0) NOT NULL DEFAULT 0 COMMENT '排序',
  `CreateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '话创建时间',
  `UpdateTime` datetime(0) NOT NULL DEFAULT '1900-01-01 00:00:00' COMMENT '话修改时间',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_HanHanCartoonPiece_HanHanCartoonId`(`HanHanCartoonId`) USING BTREE,
  INDEX `IX_HanHanCartoonPiece_HanHanCartoonChapterId`(`HanHanCartoonChapterId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 105 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '汗汗漫画话表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for hanhancartoonpieceimg
-- ----------------------------
DROP TABLE IF EXISTS `hanhancartoonpieceimg`;
CREATE TABLE `hanhancartoonpieceimg`  (
  `Id` int(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '主键Id',
  `HanHanCartoonPieceId` int(0) NOT NULL COMMENT '话表',
  `WebPageUrl` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '网页地址',
  `ImgHost` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '图片服务器地址 ',
  `WebImgUrl` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '网页图片地址',
  `LocalhostImgUrl` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT '本地图片地址',
  `Sort` int(0) NOT NULL DEFAULT 0 COMMENT '排序',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_HanHanCartoonPieceImg_HanHanCartoonId`(`HanHanCartoonPieceId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4575 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '汗汗漫画图片表' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
