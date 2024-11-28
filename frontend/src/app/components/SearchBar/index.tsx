interface SearchBarProps {
  onSearch: (value: string) => void
}

const SearchBar: React.FC<SearchBarProps> = ({ onSearch }) => {

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onSearch(event.target.value)
  }

  return (
  <div className="relative w-[255px] h-[40px]">
    <svg
      width="20"
      height="20"
      viewBox="0 0 20 20"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className="absolute left-4 top-1/2 transform -translate-y-1/2"
    >
      <g clipPath="url(#clip0_1_1602)">
        <path
          d="M19.3359 18.2109L14.7344 13.6094C15.875 12.2188 16.5625 10.4375 16.5625 8.49609C16.5625 4.04297 12.9492 0.429688 8.49609 0.429688C4.03906 0.429688 0.429688 4.04297 0.429688 8.49609C0.429688 12.9492 4.03906 16.5625 8.49609 16.5625C10.4375 16.5625 12.2148 15.8789 13.6055 14.7383L18.207 19.3359C18.5195 19.6484 19.0234 19.6484 19.3359 19.3359C19.6484 19.0273 19.6484 18.5195 19.3359 18.2109ZM8.49609 14.957C4.92969 14.957 2.03125 12.0586 2.03125 8.49609C2.03125 4.93359 4.92969 2.03125 8.49609 2.03125C12.0586 2.03125 14.9609 4.93359 14.9609 8.49609C14.9609 12.0586 12.0586 14.957 8.49609 14.957Z"
          fill="#718EBF"
        />
      </g>
      <defs>
        <clipPath id="clip0_1_1602">
          <rect width="20" height="20" fill="white" />
        </clipPath>
      </defs>
    </svg>

    <input
      type="text"
      placeholder="Search by tag ✨"
      onChange={handleChange}
      className="w-full h-full pl-12 pr-4 rounded-full bg-[#F5F7FA] text-[#8BA3CB] placeholder-[#8BA3CB] text-sm focus:outline-none"
    />
  </div>
  )
}

export default SearchBar;