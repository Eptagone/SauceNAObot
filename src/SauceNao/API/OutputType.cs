// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

namespace SauceNao.API
{
    /// <summary>
    /// 0=normal html 1=xml api(not implemented) 2=json api
    /// </summary>
    public enum OutputType
    {
        /// <summary>Normal HTML.</summary>
        NormalHtml = 0,
        /// <summary>XML Api.</summary>
        XmlApi = 1,
        /// <summary>Json Api.</summary>
        JsonApi = 2
    }
}
