﻿using System.Net;

namespace NPlatform.Result
{
    /// <summary>
    /// 返回结果的封装
    /// </summary>
    public class ResultHelper
    {

        #region success
        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual INPResult Success(string msg)
        {
            return new SuccessResult<string>(msg);
        }

        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual INPResult Success()
        {
            return new SuccessResult<string>();
        }

        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<T> Success<T>(string msg, T data)
        {
            return new SuccessResult<T>(msg, data);
        }
        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<T> Success<T>(string msg, T data, HttpStatusCode httpCode, object serializerSettings)
        {
            return new SuccessResult<T>(msg, data, httpCode, serializerSettings);
        }
        /// <summary>
        ///  返回SuccessResult
        /// </summary>
        protected virtual SuccessResult<T> Success<T>(T data)
        {
            return new SuccessResult<T>(string.Empty, data);
        }
        #endregion

        #region Error

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual INPResult Fail(string msg)
        {
            return Fail<bool>(msg);
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual FailResult<T> Fail<T>(string msg)
        {
            return new FailResult<T>(msg);
        }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual FailResult<T> FailParams<T>(string paramName)
        {
            return new FailResult<T>($"参数{nameof(paramName)}不能为空！");
        }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual FailResult<T> Fail<T>(string msg, HttpStatusCode statusCode)
        {
            return new FailResult<T>(msg, statusCode);
        }


        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual INPResult Fail(string msg, HttpStatusCode statusCode)
        {
            return new FailResult<bool>(msg, statusCode);
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        protected virtual INPResult Fail(Exception ex)
        {
            return new FailResult<bool>(ex);
        }

        /// <summary>
        /// 返回参数不能为空的提示 “{pName}参数不能为空！”
        /// 使用：NotNullResult(nameof(参数名))
        /// </summary>
        /// <param name="pName">参数名</param>
        /// <returns>IEPResult</returns>
        protected virtual INPResult NotNullError(string pName)
        {
            return Fail<bool>($"{pName}参数不能为空！");
        }


        #endregion

        /// <summary>
        /// 树格式节点
        /// </summary>
        /// <typeparam name="T">TreeNode 类型</typeparam>
        /// <param name="nodes">树节点</param>
        /// <returns></returns>
        protected TreeResult<T> TreeData<T>(IEnumerable<T> nodes)
        {
            var trees = new TreeResult<T>();
            trees.AddRange(nodes);
            return trees;
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected virtual ListResult<T> ListData<T>(IEnumerable<T> list)
        {
            var content = new ListResult<T>(list, -1);
            return content;
        }
        /// <summary>
        /// 返回数据集合
        /// </summary>
        protected virtual ListResult<T> ListData<T>(IEnumerable<T> list, long total)
        {
            var content = new ListResult<T>(list, total);
            return content;
        }

    }
}
