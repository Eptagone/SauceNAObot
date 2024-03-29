﻿// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.Data;

namespace SauceNAO.Core;

public interface ISauceDatabase
{
	IUserRepository Users { get; }
	IGroupRepository Groups { get; }
	ISauceRepository Sauces { get; }

	ITemporalFileRepository Files { get; }
}
