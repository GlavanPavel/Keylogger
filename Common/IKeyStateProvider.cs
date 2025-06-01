/*************************************************************************
 *                                                                       *
 *  File:        IKeyStateProvider.cs                                   *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *               Cojocaru Valentin                                        *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Declares the IKeyStateProvider interface which provides *
 *               a method to retrieve the state of a specific key code.  *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Provides a method to get the current state of a keyboard key by its key code.
    /// </summary>
    public interface IKeyStateProvider
    {
        /// <summary>
        /// Gets the state of the specified key.
        /// </summary>
        /// <param name="keyCode">The virtual key code of the key to query.</param>
        /// <returns>An integer representing the key's current state.</returns>
        int GetKeyState(int keyCode);
    }
}
