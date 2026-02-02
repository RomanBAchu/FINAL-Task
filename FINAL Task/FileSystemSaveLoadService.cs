using Core;
using Entities;
using System;
using System.IO;
using System.Text;

namespace Services
{
    public class FileSystemSaveLoadService<T> : ISaveLoadService<T>
    {
        private readonly string _path;

        public FileSystemSaveLoadService(string path)
        {
            _path = path;
            try
            {
                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось создать директорию {_path}: {ex.Message}", ex);
            }
        }

        public void SaveData(T data, string identifier)
        {
            string filePath = Path.Combine(_path, $"{identifier}.txt");

            try
            {
                string content = ConvertToText(data);
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Нет прав на запись в файл {filePath}: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка записи в файл {filePath}: {ex.Message}", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException($"Тип {typeof(T)} не поддерживается при сохранении: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Неожиданная ошибка при сохранении данных: {ex.Message}", ex);
            }
        }

        public T LoadData(string identifier)
        {
            string filePath = Path.Combine(_path, $"{identifier}.txt");

            if (!File.Exists(filePath))
                return default(T);

            try
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                return ConvertFromText(content);
            }
            catch (FileNotFoundException ex)
            {                
                return default(T);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException($"Нет прав на чтение файла {filePath}: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Ошибка чтения файла {filePath}: {ex.Message}", ex);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Ошибка формата данных в файле {filePath}: {ex.Message}", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException($"Тип {typeof(T)} не поддерживается при загрузке: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Неожиданная ошибка при загрузке данных: {ex.Message}", ex);
            }
        }

        private string ConvertToText(T data)
        {
            if (data is Player player)
            {
                return $"{player.Name}|{player.Bank}";
            }
            throw new NotSupportedException($"Тип {typeof(T)} не поддерживается для конвертации в текст.");
        }

        private T ConvertFromText(string content)
        {
            if (typeof(T) == typeof(Player))
            {
                try
                {
                    string[] parts = content.Split('|');
                    if (parts.Length < 2)
                        throw new FormatException("Недостаточно данных для создания Player.");

                    string name = parts[0];
                    int bank = int.Parse(parts[1]);

                    return (T)(object)new Player(name, bank);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Ошибка парсинга данных Player из строки '{content}': {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка создания Player из строки '{content}': {ex.Message}", ex);
                }
            }

            throw new NotSupportedException($"Тип {typeof(T)} не поддерживается для конвертации из текста.");
        }
    }
}
