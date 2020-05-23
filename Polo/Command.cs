﻿namespace Polo
{
    public interface ICommand
    {
        public string Name { get; }
        public string ShortName { get; }
        public string Description { get; }
        public void Action();
    }
}